<template>
<b-container class="bv-example-row">
  <b-row>
    <b-col>
      <b-card bg-variant="dark" text-variant="white" title="VK Wall Posts and comments">
        <b-card-text>
          <b-row class="my-1">
            <b-col sm="3">
              <label for="name">Community to crawl:</label>
            </b-col>
            <b-col sm="9">
              <b-form-input id="name" v-model="name" placeholder=""></b-form-input>
            </b-col>
          </b-row>
          <b-row class="my-1">
            <b-col sm="3">
              <label for="pumpHistoryMonths">Months of history:</label>
            </b-col>
            <b-col sm="9">
              <b-form-input id="pumpHistoryMonths" v-model="pumpHistoryMonths" type="number"></b-form-input>
            </b-col>
          </b-row>
        </b-card-text>
        <b-button v-on:click="initVk" href="#" variant="primary">Crawl VK community</b-button>
      </b-card>
    </b-col>
    <b-col></b-col>
  </b-row>
</b-container>

</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

@Component
export default class DataSources extends Vue {
  initVk (): void {
    const source = 'Vk';
    const props = encodeURIComponent(JSON.stringify({
      CommunityName: this.name,
      PumpHistoryMonths: this.pumpHistoryMonths
    }));
    window.open(`${process.env.VUE_APP_API_URL}/callback/${source}/redirect?props=${props}`, '_blank');
  }

  name = ''
  pumpHistoryMonths = 1
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
