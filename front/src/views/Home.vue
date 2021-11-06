<template>
  <div class="home">
    <DataSources msg="Welcome to Your Vue.js + TypeScript App"/>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import DataSources from '@/components/DataSources.vue'; // @ is an alias to /src

@Component({
  components: {
    DataSources
  }
})
export default class Home extends Vue {}
</script>
