<template>
  <div>
    <b-form-input v-model="name" placeholder="Community to crawl"></b-form-input>
    <b-button v-on:click="addVkTenant">Crawl a VK community</b-button>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

@Component
export default class HelloWorld extends Vue {
  addVkTenant (): void {
    const source = 'VkWall';
    const props = encodeURIComponent(JSON.stringify({ CommunityName: this.name }));
    console.dir(process.env);
    window.open(`${process.env.VUE_APP_API_URL}/callback/${source}/redirect?props=${props}`, '_blank');
  }

  name = ''

  data (): unknown {
    return {
      name: ''
    };
  }
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
