<template>
<b-container class="bv-example-row">
  <b-row>
    <b-col>
      <b-card bg-variant="dark" text-variant="white" title="VK Wall Posts and Comments">
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
              <label for="pumpHistoryDays">Days of history:</label>
            </b-col>
            <b-col sm="9">
              <b-form-input id="pumpHistoryDays" v-model="pumpHistoryDays" type="number"></b-form-input>
            </b-col>
          </b-row>
        </b-card-text>
        <b-button v-on:click="initVk" href="#" variant="primary">Crawl VK community</b-button>
      </b-card>
    </b-col>
    <b-col>
      <b-card bg-variant="dark" text-variant="white" title="Viber Chat">
        <b-card-text>
           <b-row class="my-1">
            <b-col sm="3">
              Download Agent
            </b-col>
            <b-col sm="9">
              <a v-bind:href="agentExeUrl" target="_blank">Download</a>
            </b-col>
          </b-row>
          <b-row class="my-1">
            <b-col sm="3">
              <label for="name">Chat name:</label>
            </b-col>
            <b-col sm="9">
              <b-form-input id="name" v-model="name" placeholder=""></b-form-input>
            </b-col>
          </b-row>
          <b-row class="my-1">
            <b-col sm="3">
              <label for="name">Phone number in international format:</label>
            </b-col>
            <b-col sm="9">
              <b-form-input id="phone" v-model="phone" placeholder="e.g., 7(999) 123 33 44"></b-form-input>
            </b-col>
          </b-row>
        </b-card-text>
        <b-button v-on:click="initViber" href="#" variant="primary">Connect Viber chat</b-button>
      </b-card>
    </b-col>
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
      PumpHistoryDays: this.pumpHistoryDays
    }));
    window.open(`${process.env.VUE_APP_API_URL}/callback/${source}/redirect?props=${props}`, '_blank');
  }

  initViber (): void {
    const source = 'Viber';
    const props = encodeURIComponent(JSON.stringify({
      ChatName: this.name,
      PhoneNumber: this.phone
    }));
    window.open(`${process.env.VUE_APP_API_URL}/callback/${source}/redirect?props=${props}`, '_blank');
  }

  name = ''
  pumpHistoryDays = 1
  phone = ''
  agentExeUrl = `${process.env.VUE_APP_API_URL}/agent/download/Consensus.Agent.exe`
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
